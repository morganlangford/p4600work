#include <stdio.h>

int main() 
{
	float num1;
	float num2;
	float total;

	printf("Enter number 1\n");
	scanf("%f",&num1);
	printf("Enter number 2\n");
	scanf("%f",&num2);

	total = num1 + num2;

	printf("Total of two numbers is %f",total);

	return 0;
}
