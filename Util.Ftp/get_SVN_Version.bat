::-----------------------------
::** 初始化
@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

REM 初始化脚本参数

SET workDir=%1
SET template=%2
SET target=%3

REM 进入主过程
GOTO MAIN
::=============================

::-----------------------------
::** 主过程
:MAIN

pushd %workDir%
SET workDir=.\

REM 检查参数
IF %workDir%=="" GOTO ARGUMENT_ERROR
IF %template%=="" GOTO ARGUMENT_ERROR
IF %target%=="" GOTO ARGUMENT_ERROR

REM 调用 TSVN 替换模板
"SubWCRev.exe" %workDir% %template% %target%

IF NOT %ERRORLEVEL% == 0 GOTO UNKNOW_ERROR
GOTO SUCESSED
::=============================

::-----------------------------
::** 错误处理

:ARGUMENT_ERROR
ECHO 传入的参数无效。
GOTO FAIL

:UNKNOW_ERROR
ECHO 生成程序集信息出现未知错误。
:FAIL
::=============================

::-----------------------------
::** 退出程序
:FAIL
ECHO 生成程序集信息失败。
popd
pause
EXIT 1

:SUCESSED
ECHO 生成程序集信息成功。
popd
::=============================