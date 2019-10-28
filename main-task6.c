#include <stdio.h>
#include <stdlib.h>
#include <math.h>	
#include <visa.h>

#include "modData.h"
#include "functionGen.h"
#include "oscilloscope.h"

void main(int argc, char** argv)
{
	unsigned char resultBuffer[256];				// holds input values before conversion
	ViStatus status = VI_SUCCESS;
	ViFindList resourceList;
	ViUInt32 num_inst;
	ViUInt32 resultCount;

	ViSession defaultRM, scopeHandle, FGHandle;		// scopeHandle, FGHandle
	ViChar description[VI_FIND_BUFLEN];
	char dataBuffer[2500];
	float floatBuffer[2500];
	float y;
	float input[1000];
	float output[1000];

	status = viOpenDefaultRM(&defaultRM);

	if(status == VI_SUCCESS)
	{
		status = viFindRsrc(defaultRM,"USB[0-9]::0x0699?*INTSR",&resourceList,&num_inst,description);
		if(status == VI_SUCCESS)
		{
			status = viOpen(defaultRM,description,VI_NULL,VI_NULL,&scopeHandle);
			if(status == VI_SUCCESS)
			{	
				status = viFindRsrc(defaultRM,"USB[0-9]::0x1AB1?*INTSR",&resourceList,&num_inst,description);
				if(status == VI_SUCCESS)
				{
					status = viOpen(defaultRM,description,VI_NULL,VI_NULL,&FGHandle);
					if(status == VI_SUCCESS)
					{	
						/* Connecting to oscilloscope and function gen */
						printf("\nOpened %s\n",description);

						identify(ViSession scopeHandle, unsigned char resultBuffer, ViUInt32 resultCount);
						identify(ViSession FGHandle, unsigned char resultBuffer, ViUInt32 resultCount);
						
						printf("\nResult count = %d",resultCount);
						printf("\nResult buffer = %s\n",resultBuffer);

						ifOpened(ViSession scopeHandle, ViUInt32 resultCount);

						/* Get curve (data points) */ 
						getCurve(scopeHandle, dataBuffer, 2500); 

						/* Get scale of scope */
						ret(ViSession scopeHandle, ViUInt32 resultCount); 
						printf("Scale is %s\n",ret);

						float volts;
						sscanf(ret,"%f",&volts);
						printf("\nvolts = %f",volts);

						for(int i = 0; i < 1000; i++)
						{
							floatBuffer[i] = dataBuffer[i];
							y = floatBuffer[i] * 10.0 * volts / 256; 	
							input[i] = y;	
							printf("\n%f",y);
						}
						float amp = amplitude(1000, output);
						printf("\nAmplitude = %f",amp);

						/* Now we need to actually smooth the curve and find the amplitude */
						printf("\nSmooth curve data:\n");
						smoothCurve(1000, input, output);
						for (int j = 0; j < 1000; j++){
							printf("\n%f", output[j]);
						}				

						float amp = amplitude(1000, output);
						printf("\nAmplitude = %f",amp);
					}
					else printf("\nFailed to open %s",description);
				}
			else printf("\nFailed to open %s",description);
		}
		else printf("\nCouldn't find any instruments");
	}
	else printf("\nFailed to open defaultRM");
}