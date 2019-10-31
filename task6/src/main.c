#include <stdio.h>
#include <stdlib.h>
#include <math.h>	
#include <visa.h>

#include "modData.h"
#include "functionGen.h"
#include "oscilloscope.h"

void main(int argc, char** argv)
{
	/* Initialize */
	unsigned char resultBuffer[256];		// holds input values
	ViStatus status = VI_SUCCESS;			
	ViFindList resourceList;
	ViUInt32 num_inst;
	ViUInt32 resultCount;

	/* Can change these values */
	int numPoints = 2500;				// number of points it reads from scope
	int num_points = 2000;				// number of points it'll actually graph

	/* Info for setting up scope & FG, for graphing and for data analysis */
	ViSession defaultRM, scopeHandle, FGHandle;
	ViChar description[VI_FIND_BUFLEN];
	char dataBuffer[numPoints];
	float floatBuffer[numPoints];
	float y;
	float input[num_points];
	float output[num_points];

	/* Ask user for range of freq to change and for data file name */
	char lower_limit_string[20];		// 20 char array for user to type into
	char upper_limit_string[20];
	char num_points_string[10];			// num points they want plotted
	char file_name[20];

    printf("\nPlease type lower limit of range of frequencies:\n");
    fgets(lower_limit_string,20,stdin); 		// gets first 20 characters typed into keyboard
    printf("\nPlease type upper limit of range of frequencies:\n");
    fgets(upper_limit_string,20,stdin); 
    printf("\nPlease type number of points you want plotted:\n");
    fgets(num_points_string,20,stdin); 
	printf("\nPlease type file name to which you want the data to be saved (e.g. amp_vs_freq.DAT):\n");
    fgets(file_name,36,stdin); 

    /* Change values obtained to integers */
	int lower_limit = atoi(lower_limit_string);
	int upper_limit = atoi(upper_limit_string);
	int num_points = atoi(num_points_string);

    status = viOpenDefaultRM(&defaultRM);
	if(status == VI_SUCCESS)
	{
		status = viFindRsrc(defaultRM,"USB[0-9]::0x0699?*INSTR",&resourceList,&num_inst,description);
		if(status == VI_SUCCESS)
		{
			status = viOpen(defaultRM,description,VI_NULL,VI_NULL,&scopeHandle);
			if(status == VI_SUCCESS)
			{
				printf("\nOpened %s\n",description);		// only if it opened successfully

				status = viFindRsrc(defaultRM,"USB[0-9]::0x1AB1?*INSTR",&resourceList,&num_inst,description);
				if(status == VI_SUCCESS)
					{
						status = viOpen(defaultRM,description,VI_NULL,VI_NULL,&FGHandle);
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

							viWrite(FGHandle,"*IDN?\n",6,&resultCount);
							viRead(FGHandle,resultBuffer,256,&resultCount);

							printf("\nResult count = %d",resultCount);
							printf("\nResult buffer = %s\n",resultBuffer);

							viWrite(FGHandle,"DAT:SOU CH1\n",12,&resultCount);
							viWrite(FGHandle,"DAT:START 1\n",12,&resultCount);
							viWrite(FGHandle,"DAT:STOP 2500\n",14,&resultCount);		// don't forget to change the number of points


							/* Get curve (data points) */ 
							ViUInt32 resultCount;
							ViStatus status;
							viWrite(scopeHandle, "CURV?\n",6,&resultCount);
							status = viRead(scopeHandle, dataBuffer, numPoints, &resultCount); 

							/* Get scale of scope */
							char ret[20];			// return
							ret[19] = '\0';
							viWrite(scopeHandle,"CH1:SCA?\n",9,&resultCount);
							viRead(scopeHandle,ret,20,&resultCount);
							printf("Scale is %s\n",ret);

							float volts;
							sscanf(ret,"%f",&volts);
							printf("\nvolts = %f",volts);

							for(int i = 0; i < num_points; i++){
								floatBuffer[i] = dataBuffer[i];
								y = floatBuffer[i] * 10.0 * volts / 256; 	
								input[i] = y;	
								//printf("\n%f",y);
							}

							/* Now we need to actually smooth the curve and find the amplitude */
							//printf("\nSmooth curve data:\n");
							//smoothCurve(num_points, input, output);
							
							cycleFreq(lower_limit, upper_limit, num_points, file_name, input);
						}
					else printf("\nFailed to open %s",description);
				}
				else printf("\nCouldn't find any instruments");
			}
			else printf("\nFailed to open %s",description);
		}
		else printf("\nCouldn't find any instruments");
	}
	else printf("\nFailed to open defaultRM");
}