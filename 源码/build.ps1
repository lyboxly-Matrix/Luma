$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
$buildDir=Join-Path $PSScriptRoot 'build'
$engine=Join-Path $root '.luma-runtime\engine-20261015\mpv.exe'
if(!(Test-Path -LiteralPath $engine)){
 New-Item -ItemType Directory -Force $buildDir | Out-Null
$download=@'
const fs=require('fs');
async function run(){
 const url='https://github.com/shinchiro/mpv-winbuild-cmake/releases/download/20261015/mpv-x86_64-20261015-git-492bae0768.7z';
 const b=Buffer.from(await (await fetch(url)).arrayBuffer());
 if(require('crypto').createHash('sha256').update(b).digest('hex')!=='f9becefe40e2a29d2ce5f3ab231ff094f9cf3e4038ff014cfe4a0c09ad90e79e')throw Error('Hash mismatch');
 fs.writeFileSync(process.argv[2]+'/mpv.7z',b);
 fs.writeFileSync(process.argv[2]+'/7zr.exe',Buffer.from(await (await fetch('https://www.7-zip.org/a/7zr.exe')).arrayBuffer()));
}
run().catch(e=>{console.error(e);process.exit(1)});
'@
$download | Set-Content (Join-Path $buildDir 'download.js') -Encoding UTF8
node (Join-Path $buildDir 'download.js') $buildDir
if($LASTEXITCODE -ne 0){throw 'Download failed'}
& (Join-Path $buildDir '7zr.exe') x (Join-Path $buildDir 'mpv.7z') "-o$buildDir\engine" -y
if($LASTEXITCODE -ne 0){throw 'Extraction failed'}
 $engine=Join-Path $buildDir 'engine\mpv.exe'
}
$compiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $compiler /nologo /target:winexe /platform:x64 /optimize+ "/out:$root\Luma.exe" /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Web.Extensions.dll "/resource:$engine,mpv.exe" (Join-Path $PSScriptRoot 'Player.cs')
if($LASTEXITCODE -ne 0){throw 'Compilation failed'}


Write-Host "构建完成：$root\Luma.exe"
