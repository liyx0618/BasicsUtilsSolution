::-----------------------------
::** ��ʼ��
@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

REM ��ʼ���ű�����

SET workDir=%1
SET template=%2
SET target=%3

REM ����������
GOTO MAIN
::=============================

::-----------------------------
::** ������
:MAIN

pushd %workDir%
SET workDir=.\

REM ������
IF %workDir%=="" GOTO ARGUMENT_ERROR
IF %template%=="" GOTO ARGUMENT_ERROR
IF %target%=="" GOTO ARGUMENT_ERROR

REM ���� TSVN �滻ģ��
"SubWCRev.exe" %workDir% %template% %target%

IF NOT %ERRORLEVEL% == 0 GOTO UNKNOW_ERROR
GOTO SUCESSED
::=============================

::-----------------------------
::** ������

:ARGUMENT_ERROR
ECHO ����Ĳ�����Ч��
GOTO FAIL

:UNKNOW_ERROR
ECHO ���ɳ�����Ϣ����δ֪����
:FAIL
::=============================

::-----------------------------
::** �˳�����
:FAIL
ECHO ���ɳ�����Ϣʧ�ܡ�
popd
pause
EXIT 1

:SUCESSED
ECHO ���ɳ�����Ϣ�ɹ���
popd
::=============================