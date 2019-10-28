#include <stdio.h>
#include <stdlib.h>
#include <math.h>	
#include <visa.h>

#include "modData.h"
#include "functionGen.h"
#include "oscilloscope.h"

void main(int argc, char** argv)
{
	unsigned char resultBuffer[256];		// holds input values
	ViStatus status = VI_SUCCESS;
	ViFindList resourceList;
	ViUInt32 num_inst;
	ViUInt32 resultCount;

	/* Set these values */
	int numPoints = 2500;		// number of points it reads from scope
	int graphPoints = graphPoints;		// number of points it'll actually graph

	ViSession defaultRM, scopeHandle, FGHandle;
	ViChar description[VI_FIND_BUFLEN];
	char dataBuffer[numPoints];
	float floatBuffer[numPoints];
	float y;
	float input[graphPoints];
	float output[graphPoints];

	char lower_limit_string[20];
    printf("\nPlease type lower limit of range of frequencies:\n");
    fgets(lower_limit_string,20,stdin); 
    int lower_limit = atoi(lower_limit_string);

    char upper_limit_string[20];
    printf("\nPlease type upper limit of range of frequencies:\n");
    fgets(upper_limit_string,20,stdin); 
    int upper_limit = atoi(upper_limit_string);

    char file_name[20];
    printf("\nPlease type file name to which you want the data to be saved:\n");
    fgets(file_name,20,stdin); 

	status = viOpenDefaultRM(&defaultRM);
	if(status == VI_SUCCESS)
	{
		status = viFindRsrc(defaultRM,"USB[0-9]::0x0699?*INSTR",&resourceList,&num_inst,description);
		if(status == VI_SUCCESS)
		{
			status = viOpen(defaultRM,description,VI_NULL,VI_NULL,&scopeHandle);
			if(status == VI_SUCCESS)
			{
				printf("\nOpened %s\n",description);

				viWrite(scopeHandle,"*IDN?\n",6,&resultCount);
				viRead(scopeHandle,resultBuffer,256,&resultCount);

				printf("\nResult count = %d",resultCount);
				printf("\nResult buffer = %s\n",resultBuffer);

				viWrite(scopeHandle,"DAT:SOU CH1\n",12,&resultCount);
				viWrite(scopeHandle,"DAT:START 1\n",12,&resultCount);
				viWrite(scopeHandle,"DAT:STOP 2500\n",14,&resultCount);		// don't forget to change the number of points

				/* Get curve (data points) */ 
				ViUInt32 resultCount;
				ViStatus status;
				viWrite(scopeHandle, "CURV?\n",6,&resultCount);
				status = viRead(scopeHandle, dataBuffer, numPoints, &resultCount); 

				/* Get scale of scope */
				char ret[20];	// return
				ret[19] = '\0';
				viWrite(scopeHandle,"CH1:SCA?\n",9,&resultCount);
				viRead(scopeHandle,ret,20,&resultCount);
				printf("Scale is %s\n",ret);

				float volts;
				sscanf(ret,"%f",&volts);
				printf("\nvolts = %f",volts);

				for(int i = 0; i < graphPoints; i++){
					floatBuffer[i] = dataBuffer[i];
					y = floatBuffer[i] * 10.0 * volts / 256; 	
					input[i] = y;	
					printf("\n%f",y);
				}

				/* Now we need to actually smooth the curve and find the amplitude */
				printf("\nSmooth curve data:\n");
				smoothCurve(graphPoints, input, output);
				for (int j = 0; j < graphPoints; j++){
					printf("\n%f", output[j]);
				}				

				float amp = amplitude(graphPoints, output);
				printf("\nAmplitude = %f",amp);


				/* Print out frequency vs amplitude */
				FILE* freq_vs_amp;
				freq_vs_amp = fopen("plot_final_data.DAT","w");
				for(int k = 0; k < graphPoints; k++){
					fprintf(freq_vs_amp,"\n%f",output[k]);
				}

				fclose(freq_vs_amp);
			}
			else printf("\nFailed to open %s",description);
		}
		else printf("\nCouldn't find any instruments");
	}
	else printf("\nFailed to open defaultRM");
}