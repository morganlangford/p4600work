#include <stdio.h>
#include <stdlib.h>
#include <math.h>

float findMean(float* input, int len){
	printf("\nMade it to findMean");
	float mean = 0;

	for (int i=0; i<len; i++){
		mean += input[i];
	}

	mean = mean/len;
	printf("\nMean = %f",mean);

	return mean;
}

float findStdDev(float* input, float mean, int len){
	float sum;

	for (int j=0; j<len; j++){
		sum += powf((input[j] - mean),2);
	}

	float stdDev = sqrt(sum/(len-1));
	printf("Standard deviation = %f",stdDev);

	return stdDev;
}

int main(){
	FILE* dataFile;	// pointer towards our data file holding x and y for sine

	int counter = 0;
	float x; 
	float z;

	dataFile = fopen("sineWave.dat","r");	// open file holding sine values

	if (dataFile != NULL){ 					// Did the file successfully open? Otherwise DON'T WRITE 
		while (fscanf(dataFile,"%f %f",&x,&z) != EOF){ // counting how many rows are in the file
			counter++;
		}
		//printf("Counter = %i",counter);
		float y[counter]; 					// creating an array of length of file

		rewind(dataFile); 					// start at the beginning of the file again

		for (int i = 0; i<counter; i++){	// put all y values into an array called y
			fscanf(dataFile,"%f %f",&x,&y[i]);
		}
		printf("\nmade it here");
		float mean = findMean(y,counter); 	// send array y and length to function findMean
		findStdDev(y,mean,counter);		// call next function to find standard dev

		
	}
	else printf("\nCouldn't read file.");	// uf the file couldn't be opened...

	fclose(dataFile);						// close file
}