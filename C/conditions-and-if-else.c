#include <stdio.h>
#include <string.h>

int main()
{
	int number;
	printf("Enter the number:\n");
	scanf("%d",&number);

	if (number < 0)
	{
		printf("The number is negative");
	}
	else printf("The number is positive");

	return 0;
}
