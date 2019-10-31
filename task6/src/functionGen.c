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