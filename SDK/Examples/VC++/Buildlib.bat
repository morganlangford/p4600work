cd \"program files"\"Microsoft Visual Studio"\vc98\bin
dumpbin /EXPORTS mightex_leddriver_sdk.dll > mightex_leddriver_sdk.def
lib /def:mightex_leddriver_sdk.def