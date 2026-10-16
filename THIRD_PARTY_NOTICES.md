# 第三方组件

## mpv

Luma 将 mpv Windows 播放内核嵌入 EXE，运行时释放为独立进程。mpv 使用 GPL-2.0-or-later，可能包含具有各自许可的依赖库；版权与许可说明见 mpv-Copyright 和上游源码。

- Windows 构建发布：https://github.com/shinchiro/mpv-winbuild-cmake/releases/tag/20261015
- 构建脚本及依赖配置：https://github.com/shinchiro/mpv-winbuild-cmake
- mpv 对应源码：https://github.com/mpv-player/mpv/tree/492bae0768
- mpv 许可：https://github.com/mpv-player/mpv/blob/492bae0768/Copyright

锁定归档：mpv-x86_64-20261015-git-492bae0768.7z。

## 7-Zip

构建脚本在需要下载内核时获取 7zr.exe，用于解压 7z 归档，不打包进 Luma.exe。

- 官网：https://www.7-zip.org/
- 许可说明：https://www.7-zip.org/license.txt
