#include <stdio.h>
#include <stdlib.h>
#include <visa.h>
#include <string.h>

/* Function generator */
// Found this in Chapter 2 of RIGOL manual, pg 90/264 

// This function will change the frequency of the FG

void changeFreq(ViSession * handle, int freq, float peak_to_peak_volts){
	ViUInt32 = resultCount;
	char request[25];
	sprintf(request,":SOUR1:APPL:SIN %f,%f", freq,peak_to_peak_volts);
	viWrite(*handle, request, strlen(request),&resultCount);

}

void getScale(char* ret)
{
	viWrite(scopeHandle,"CH1:SCA?\n",9,&resultCount); //request the scale for the scope
	viRead(scopeHandle,ret,20,&resultCount);		//places it in a string called ret
}


/* Haven't actually used this yet */
void getCurve(ViSession handle, char* curveArray)
{
	viWrite(handle,"DAT:SOU CH1\n",12,&resultCount);
	viWrite(handle,"CURV?\n",6,&resultCount);
	_sleep(2);
	viRead(handle,curveArray,2500,&resultCount); //returns a signed integer
}