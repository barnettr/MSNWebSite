!INCLUDE $(INETROOT)\build\makefile.inc

CopyLibsPreBuild:
!if "$(LINKONLY)" == "1"
    :: Copying Utilities.dll and HSLDTOs.dll to the HSLDownloads Bin folder
    IF NOT EXIST .\WebSite\Bin mkdir .\WebSite\Bin
    copy /Y $(TARGETPATH)\$(TARGET_DIRECTORY)\Utilities.dll .\WebSite\Bin\ > filecopy.log
    copy /Y $(TARGETPATH)\$(TARGET_DIRECTORY)\HSLDTOs.dll .\WebSite\Bin\ >> filecopy.log
    copy /Y $(INETROOT)\private\extern\AntiXssLibrary\AntiXssLibrary.dll .\WebSite\Bin\ >> filecopy.log
!endif

CopyLibsPostBuild:
!if "$(LINKONLY)" == "1"
    :: Copying HSLDownloads bits to target
    copy /Y $(ASPNET_TARGET)\bin\*.dll $(TARGETPATH)\$(TARGET_DIRECTORY) >> filecopy.log
!endif
