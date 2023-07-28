# arca
API Response Comparison Assistant

# Download installation
Download the tool from [here](TODO), or build it from source code cloning the repository. The output is a single executable.

# Configuration
The tool basically calls 2 endpoints with the same parameters and ensures that the answers contains the same field. You need to configure 2 files: 
- a JSON configuration file (default **config.json** in the same folder of the executable)
- a JSON descriptive that will be contains the list of API calls to be compared (default **arca.json**)

# Launch
Simply invoke it with
```bash
arca.exe
```

## Parameters
|Parameter|Description|Default|Example|
|-|-|-|-|
|-c|Configuration file|config.json|arca.exe -c otherconfig.json|
|-a|API calls file|arca.json|arca.exe -a otherarca.json|
|-f|Output format file|csv|arca.exe -f csv|
|-o|Output file|report.csv|arca.exe -o myreport.csv|

Full command:
```bash
arca.exe -c otherconfig.json -a otherarca.json -f csv -o myreport.csv
```

# ARCA file format
Basically all the software does is calling first api on first environment, then calls the second api on second environment, and compare the results. A different response code and/or body will be signaled as error.


## Json format
```json
{
  "define": [

  ],
"calls": [
  "name": "Get users",
  "onA":
  {
    "type": "GET",
    "params:" [],
    "url": "{BASE_A}/users"
  },
  "onB":
  {
    "type": "GET",
    "params": [ {"filter": "ALL", "page": "false"  }]
    "url": "{BASE_B}/user" 
  }
]
}

```

## Arca format
- Put your variables with *<name> = <value>* format wrapped into a *#define/#enifed* block at the begininning of your script
- Empty lines are skipped, as long as lines starting with *//* (comments)
- Every pair of API call to compare starts with "*" and the name of the pair
- Define the call on first environment (A) with *onA:* followed by the definition. Do the same for *onB:*
- Definition follow the schema *<type> <url> <body>*, parameters must be inserted directly in url (you can use variables defined with *$<variable name>$*)

```
#define
BASE_A = https://apidns1:7286/api/v2
BASE_B = https://apidns2:7286/api/v2

TEST_USER_ID = 23

BD_CREATE_USER = {"name":"Paolo","email":"paolo.cattaneo@mymail.com","age":31}
#enifed

// Comment example

* Get users
onA: GET $BASE_A$/users
onB: GET $BASE_B$/user?filter=ALL&page=false

* Get specific user
onA: GET $BASE_A$/users/$TEST_USER_ID$
onB: GET $BASE_B$/user?filter=$TEST_USER_ID$

*! Create an user
onA: POST $BASE_A$/users $BD_CREATE_USER$
onB: POST $BASE_B$/users {"name":"Paolo","email":"paolo.cattaneo@mymail.com","age":31}

* Delete user
onA: DELETE $BASE_A$/users/52
onB: DELETE $BASE_B$/user?filter=52

```





