# Diagnostic image layout only: no retouching, grading, or generative image edits.
param([string]$Stage = 'pass2')
Add-Type -AssemblyName System.Drawing
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$faceRoot = Join-Path $repoRoot 'artifacts/master-human-poc2a/face'
$baselineRoot = Join-Path $repoRoot 'artifacts/master-human-poc/renders'
$comparisonRoot = Join-Path $faceRoot "$Stage/comparisons"
New-Item -ItemType Directory -Path $comparisonRoot -Force | Out-Null
$pairs = @(
    @('01_face_front', '03_face_neutral'),
    @('02_face_three_quarter', '12_face_three_quarter'),
    @('03_face_profile', '04_face_profile'),
    @('04_face_one_meter', '13_one_meter_view'),
    @('05_eye_closeup', '14_eyes_closeup'),
    @('06_mouth_closeup', '15_mouth_closeup'),
    @('07_hairline_closeup', '16_hairline_closeup'),
    @('08_skin_grazing_light', '17_face_grazing_light')
)
$font = New-Object System.Drawing.Font('Segoe UI', 17)
$small = New-Object System.Drawing.Font('Segoe UI', 11)
$white = [System.Drawing.Brushes]::White
function Draw-Fit($graphics, $imagePath, $x, $y, $w, $h) {
    $img = [System.Drawing.Image]::FromFile($imagePath)
    try {
        $scale = [Math]::Min($w / $img.Width, $h / $img.Height)
        $dw = [int]($img.Width * $scale); $dh = [int]($img.Height * $scale)
        $graphics.DrawImage($img, [int]($x+($w-$dw)/2), [int]($y+($h-$dh)/2), $dw, $dh)
    } finally { $img.Dispose() }
}
foreach ($pair in $pairs) {
    $canvas = New-Object System.Drawing.Bitmap(1800, 1000)
    $graphics = [System.Drawing.Graphics]::FromImage($canvas)
    try {
        $graphics.Clear([System.Drawing.Color]::FromArgb(34,34,34))
        $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $graphics.DrawString('POC-1 original render', $font, $white, 20, 10)
        $graphics.DrawString("Face-only $Stage", $font, $white, 920, 10)
        Draw-Fit $graphics (Join-Path $baselineRoot ($pair[1]+'.png')) 0 55 900 900
        Draw-Fit $graphics (Join-Path $faceRoot "$Stage/$($pair[0]).png") 900 55 900 900
        $graphics.DrawString('Unretouched. Lighting differs; front and three-quarter share camera parameters. Profile/detail/1m framing differs.', $small, $white, 20, 970)
        $canvas.Save((Join-Path $comparisonRoot ($pair[0]+'_vs_poc1.png')), [System.Drawing.Imaging.ImageFormat]::Png)
    } finally { $graphics.Dispose(); $canvas.Dispose() }
}
$canvas = New-Object System.Drawing.Bitmap(1800, 1860)
$graphics = [System.Drawing.Graphics]::FromImage($canvas)
try {
    $graphics.Clear([System.Drawing.Color]::FromArgb(34,34,34))
    $graphics.DrawString("Face-only $Stage / all eight unretouched diagnostics", $font, $white, 20, 12)
    for ($i=0; $i -lt $pairs.Count; $i++) {
        $x = ($i % 3)*600; $y = [Math]::Floor($i / 3)*600+55
        $graphics.DrawString($pairs[$i][0], $small, $white, [int]$x+12, [int]$y)
        Draw-Fit $graphics (Join-Path $faceRoot "$Stage/$($pairs[$i][0]).png") $x ($y+28) 600 565
    }
    $canvas.Save((Join-Path $faceRoot "$Stage/contact_sheet.png"), [System.Drawing.Imaging.ImageFormat]::Png)
} finally { $graphics.Dispose(); $canvas.Dispose(); $font.Dispose(); $small.Dispose() }
Write-Output "Comparisons saved: $comparisonRoot"
