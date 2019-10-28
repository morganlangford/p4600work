/* oscilloscope */
#include <stdio.h>
#include <visa.h>


ViStatus getCurve(ViSession handle, char* dataBuffer, int npoints)
{
	ViUInt32 resultCount;
	ViStatus status;
	viWrite(handle, "CURV?\n",6,&resultCount);
	status = viRead(handle, dataBuffer, npoints, &resultCount);
	return status;
}

void identify(ViSession scopeHandle, char* resultBuffer, ViUInt32 resultCount)
{
	viWrite(scopeHandle,"*IDN?\n",6,&resultCount);
	viRead(scopeHandle,resultBuffer,256,&resultCount);
}

void ifOpened(ViSession scopeHandle, ViUInt32 resultCount)
{
	viWrite(scopeHandle,"DAT:SOU CH1\n",12,&resultCount);
	viWrite(scopeHandle,"DAT:START 1\n",12,&resultCount);
	viWrite(scopeHandle,"DAT:STOP 2500\n",14,&resultCount);
}

void ret(ViSession scopeHandle, ViUInt32 resultCount)
{
	char ret[20];	// return
	ret[19] = '\0';
	viWrite(scopeHandle,"CH1:SCA?\n",9,&resultCount);
	viRead(scopeHandle,ret,20,&resultCount);
}

