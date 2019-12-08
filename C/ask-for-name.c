#include <stdio.h>

int main()
{
	char first_name[10];
	char last_name[10];

	printf("Enter first name\n");
	scanf("%s",&first_name);
	printf("Enter last name\n");
	scanf("%s",&last_name);

	printf("The name of the person is %s %s", first_name, last_name);
}