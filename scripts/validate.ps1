#!/usr/bin/env pwsh
#
# Modernization Hackathon — Validation Script
# Checks all 9 functional requirements and outputs a scoreboard.
#
# Usage:
#   ./scripts/validate.ps1
#   ./scripts/validate.ps1 -ProjectPath ./src/LegacyInventoryApi
#

param(
    [string]$ProjectPath = "src/LegacyInventoryApi",
    [int]$Port = 5150,
    [int]$StartupWaitSeconds = 8
)

$ErrorActionPreference = "Continue"
$score = 0
$maxScore = 90
$results = @()

function Test-Requirement {
    param([string]$Id, [string]$Name, [scriptblock]$Check)
    
    Write-Host "`n  [$Id] $Name" -NoNewline
    try {
        $passed = & $Check
        if ($passed) {
            Write-Host " ✅ PASS" -ForegroundColor Green
            $script:score += 10
            $script:results += [PSCustomObject]@{ Id = $Id; Requirement = $Name; Points = 10; Status = "PASS" }
        } else {
            Write-Host " ❌ FAIL" -ForegroundColor Red
            $script:results += [PSCustomObject]@{ Id = $Id; Requirement = $Name; Points = 0; Status = "FAIL" }
        }
    } catch {
        Write-Host " ❌ FAIL ($_)" -ForegroundColor Red
        $script:results += [PSCustomObject]@{ Id = $Id; Requirement = $Name; Points = 0; Status = "FAIL" }
    }
}

# Resolve project path
$repoRoot = git rev-parse --show-toplevel 2>$null
if (-not $repoRoot) { $repoRoot = Get-Location }
$projectFullPath = Join-Path $repoRoot $ProjectPath
$csprojFiles = Get-ChildItem $projectFullPath -Filter "*.csproj" -Recurse -ErrorAction SilentlyContinue

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  MODERNIZATION HACKATHON VALIDATOR" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nProject: $projectFullPath"

# ─── Requirement 1: Targets net10.0 ───
Test-Requirement -Id "1" -Name "Targets net10.0" -Check {
    $csprojContent = Get-Content $csprojFiles[0].FullName -Raw
    $csprojContent -match "net10\.0"
}

# ─── Requirement 2: Builds successfully ───
Test-Requirement -Id "2" -Name "Builds successfully (dotnet build)" -Check {
    $buildOutput = dotnet build $projectFullPath --nologo 2>&1
    $LASTEXITCODE -eq 0
}

# ─── Requirement 3: Runs and responds (tested after starting the app) ───
Write-Host "`n  Starting application for endpoint tests..." -ForegroundColor Yellow
$appProcess = $null
$appRunning = $false

try {
    $appProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project $projectFullPath --urls http://localhost:$Port" -PassThru -WindowStyle Hidden
    Start-Sleep -Seconds $StartupWaitSeconds
    
    if (-not $appProcess.HasExited) {
        try {
            $null = Invoke-RestMethod "http://localhost:$Port/api/products" -TimeoutSec 5
            $appRunning = $true
        } catch {
            Start-Sleep -Seconds 3
            try {
                $null = Invoke-RestMethod "http://localhost:$Port/api/products" -TimeoutSec 5
                $appRunning = $true
            } catch { }
        }
    }
} catch { }

Test-Requirement -Id "3" -Name "Runs and responds on http://localhost:$Port" -Check {
    $appRunning
}

# ─── Requirement 4: Startup.cs removed, minimal hosting ───
Test-Requirement -Id "4" -Name "Startup.cs removed, minimal hosting used" -Check {
    $startupFiles = Get-ChildItem $projectFullPath -Filter "Startup.cs" -Recurse -ErrorAction SilentlyContinue
    $startupFiles.Count -eq 0
}

# ─── Requirement 5: Newtonsoft.Json removed ───
Test-Requirement -Id "5" -Name "Newtonsoft.Json removed, System.Text.Json used" -Check {
    $allFiles = Get-ChildItem $projectFullPath -Include "*.cs","*.csproj" -Recurse -ErrorAction SilentlyContinue
    $newtonsoftRefs = $allFiles | Select-String -Pattern "Newtonsoft" -SimpleMatch -ErrorAction SilentlyContinue
    $newtonsoftRefs.Count -eq 0
}

# ─── Requirement 6: Swashbuckle removed, built-in OpenAPI ───
Test-Requirement -Id "6" -Name "Swashbuckle removed, built-in OpenAPI used" -Check {
    $allFiles = Get-ChildItem $projectFullPath -Include "*.cs","*.csproj" -Recurse -ErrorAction SilentlyContinue
    $swashRefs = $allFiles | Select-String -Pattern "Swashbuckle" -SimpleMatch -ErrorAction SilentlyContinue
    $swashRefs.Count -eq 0
}

# ─── Requirement 7: All endpoints return correct response shapes ───
Test-Requirement -Id "7" -Name "All endpoints return correct response shapes" -Check {
    if (-not $appRunning) { return $false }
    
    # Test GET /api/products (paged response)
    $products = Invoke-RestMethod "http://localhost:$Port/api/products" -TimeoutSec 5
    if (-not $products.success) { return $false }
    if ($null -eq $products.data.items) { return $false }
    if ($null -eq $products.data.totalCount) { return $false }
    if ($null -eq $products.data.page) { return $false }
    if ($null -eq $products.data.pageSize) { return $false }
    
    # Test GET /api/products/1 (single product)
    $product = Invoke-RestMethod "http://localhost:$Port/api/products/1" -TimeoutSec 5
    if (-not $product.success) { return $false }
    if ($null -eq $product.data.id) { return $false }
    if ($null -eq $product.data.name) { return $false }
    if ($null -eq $product.data.sku) { return $false }
    if ($null -eq $product.data.price) { return $false }
    if ($null -eq $product.data.categoryId) { return $false }
    
    # Test GET /api/categories
    $categories = Invoke-RestMethod "http://localhost:$Port/api/categories" -TimeoutSec 5
    if (-not $categories.success) { return $false }
    if ($categories.data.Count -lt 3) { return $false }
    
    # Test GET /api/products/search?q=mouse
    $search = Invoke-RestMethod "http://localhost:$Port/api/products/search?q=mouse" -TimeoutSec 5
    if (-not $search.success) { return $false }
    if ($search.data.Count -lt 1) { return $false }
    
    # Test 404 for non-existent product
    try {
        Invoke-RestMethod "http://localhost:$Port/api/products/9999" -TimeoutSec 5
        return $false  # Should have thrown
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -ne 404) { return $false }
    }
    
    # Test POST validation (400 for bad input)
    try {
        $body = @{ name = ""; price = -1; sku = ""; categoryId = 0 } | ConvertTo-Json
        Invoke-RestMethod "http://localhost:$Port/api/products" -Method Post -Body $body -ContentType "application/json" -TimeoutSec 5
        return $false  # Should have thrown
    } catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -ne 400) { return $false }
    }
    
    return $true
}

# ─── Requirement 8: Modern C# features ───
Test-Requirement -Id "8" -Name "Modern C# (file-scoped namespaces, primary constructors)" -Check {
    $csFiles = Get-ChildItem $projectFullPath -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue
    $fileScopedCount = 0
    $blockNamespaceCount = 0
    
    foreach ($file in $csFiles) {
        $content = Get-Content $file.FullName -Raw
        # File-scoped: namespace Foo.Bar;
        if ($content -match "(?m)^namespace\s+[\w.]+\s*;") { $fileScopedCount++ }
        # Block-style: namespace Foo.Bar { or namespace Foo.Bar\n{
        if ($content -match "(?m)^namespace\s+[\w.]+\s*\{" -or $content -match "(?m)^namespace\s+[\w.]+\s*\r?\n\s*\{") { $blockNamespaceCount++ }
    }
    
    ($fileScopedCount -gt 0) -and ($blockNamespaceCount -eq 0)
}

# ─── Requirement 9: Real async I/O ───
Test-Requirement -Id "9" -Name "Real async I/O (no Task.FromResult wrapping)" -Check {
    $csFiles = Get-ChildItem $projectFullPath -Filter "*.cs" -Recurse -ErrorAction SilentlyContinue
    $taskFromResult = $csFiles | Select-String -Pattern "Task\.FromResult" -SimpleMatch -ErrorAction SilentlyContinue
    $taskFromResult.Count -eq 0
}

# ─── Cleanup ───
if ($appProcess -and -not $appProcess.HasExited) {
    Stop-Process -Id $appProcess.Id -Force -ErrorAction SilentlyContinue
}

# ─── Scoreboard ───
Write-Host "`n`n========================================" -ForegroundColor Cyan
Write-Host "            SCOREBOARD" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

Write-Host "  #   Requirement                                          Pts   Status" -ForegroundColor White
Write-Host "  ─── ──────────────────────────────────────────────────── ───── ──────" -ForegroundColor DarkGray

foreach ($r in $results) {
    $color = if ($r.Status -eq "PASS") { "Green" } else { "Red" }
    $icon = if ($r.Status -eq "PASS") { "✅" } else { "❌" }
    Write-Host ("  {0,-3} {1,-52} {2,-5} {3}" -f $r.Id, $r.Requirement, $r.Points, $icon) -ForegroundColor $color
}

$passCount = ($results | Where-Object Status -eq "PASS").Count
$failCount = ($results | Where-Object Status -eq "FAIL").Count

Write-Host "`n  ────────────────────────────────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  Requirements passed: $passCount / 9" -ForegroundColor $(if ($passCount -eq 9) { "Green" } else { "Yellow" })
Write-Host "  Completeness score:  $score / $maxScore points" -ForegroundColor $(if ($score -eq $maxScore) { "Green" } else { "Yellow" })
Write-Host ""

if ($score -eq $maxScore) {
    Write-Host "  🏆 ALL REQUIREMENTS MET!" -ForegroundColor Green
    Write-Host "  Final ranking depends on token usage (lower = better)." -ForegroundColor Green
} else {
    Write-Host "  ⚠️  Some requirements not met." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "  Scoring formula: Final Score = $score - (Total Tokens / 1000)" -ForegroundColor DarkGray
Write-Host "  Submit token proof in PROOF.md to calculate final ranking." -ForegroundColor DarkGray
Write-Host "`n========================================`n" -ForegroundColor Cyan

exit $(if ($score -eq $maxScore) { 0 } else { 1 })
