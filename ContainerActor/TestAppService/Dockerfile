FROM microsoft/windowsservercore

ENV TASKID 07987ff5-502d-44a2-87f3-ba0c619ebeeb
ENV DELETEDURATION 1

ADD publish/ /
ENTRYPOINT TestAppService.exe %TASKID% %DELETEDURATION%