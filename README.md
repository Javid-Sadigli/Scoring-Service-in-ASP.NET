# Scoring-Service-in-ASP.NET

## Table of Contents 
* [Getting Started](#getting-started)
* [Technical Aspects](#technical-aspects)
* [How to add a new Condition?](#how-to-add-a-new-condition)
* [Contributing](#contributing)

## Getting Started
To get started with the application, you should go through these steps :

#### Step 1 : Clone the repository 
Firstly, you need to clone the repository by using the following commands : 
```sh
git clone https://github.com/Javid-Sadigli/Scoring-Service-in-ASP.NET.git
cd Scoring-Service-in-ASP.NET
```

#### Step 2 : Run the project
After cloning the repository, you can run the project. In the main project directory you can see `docker-compose.yaml` file. Using this file, you can run our application and MsSQL database, by typing the following command : 
```sh
docker-compose up --build -d 
``` 
This command will download the images from the corresponding resources, and will run container for each of the services (application, database).

*Note : After running docker-compose file, both application and database will be runned, but the API endpoint won't work, because in the database, you don't have the proper tables. You will first need to run the migrations to create them.*

## Technical Aspects
1. **Web API Design**
    * A RESTFul API endpoint that takes customer data as input : 

        ```json
        {
            "finCode": "string",
            "firstName": "string",
            "lastName": "string",
            "age": 0,
            "salary": 0,
            "citizenship": "string"
        }
        ```
    * The API returns a response with body of following structure :

        ```json
        {
            "customerFinCode": "string",
            "creditAmount": 0,
            "evaulationResults": [
                {
                "id": "guid",
                "conditionId": 1,
                "isSatisfied": true,
                "amount": 1000,
                "customerRequestId": "guid"
                },
                {
                "id": "guid",
                "conditionId": 2,
                "isSatisfied": false,
                "amount": 0,
                "customerRequestId": "guid"
                },
                // ...
            ]
        }
        ``` 

2. **Conditions Logic**
    * Implemented conditions as separate components or classes. Each condition:
        * Has a unique ID for troubleshooting
        * Evaluates customer data and returns:
            * Whether the condition is satisfied.
            * An optional amount.
        * Is easily configurable (e.g., via configuration files) and extensible (new conditions can be added with minimal effort).

3. **Logging and Observability**
    * Recorded the output of each condition in:
        * Logs (include condition ID, input data, and output).
        * A database for audit and troubleshooting.

4. **Database Integration**
    * Designed a database schema to store:
        * Customer scoring request details.
        * Individual condition results (condition ID, input data, output).

5. **Code Quality**
    * Used clean, modular, and testable code.
    * Written unit tests for individual conditions and integration tests for the API.

## How to add a new Condition?
Since the conditions are easily extensible, you only need a simple few steps to add new one : 
### 1) Add the configuration (represents the businnes rule): 
In the main project, you can see `appsettings.json` file. Here, there are the configurations of existing conditions. You should add the configuration of the new condition here : 
```json
 "Application": {
        "Conditions": {
            "AgeCondition": {
                "Min": 18,
                "Max": 65,
                "CreditAmount": 1000
            },
            "CitizenshipCondition": {
                "Value": "AZE",
                "CreditAmount": 500
            },
            "SalaryCondition": {
                "Min": 1000,
                "CreditAmount": 2000
            }

            // Your condition 
            "CustomCondition": {
                // Define here the configuration of condition.
            }
        }
    }
```

After adding the configuration into `appsettings.json` file, you need to read it from there inside code. That's why you should create a new configuration class inside `Configuration\Conditions` folder : 
```cs
public class CustomConditionConfiguration
{
    // Each condition must have a unique id.
    public int ConditionId { get; set; } = 4;
    
    // The parameters from appsettings.json file. 
}   
```

### 2) Create the condition class :
Now, you can create the condition class : 
```cs
public class CustomCondition : ICondition
{
    public int Id {  get; }

    private readonly CustomConditionConfiguration configuration;

    public SalaryCondition(IOptions<CustomConditionConfiguration> configuration)
    {
        this.configuration = configuration.Value;
        this.Id = this.configuration.ConditionId; 
    }

    public ConditionEvaulationResult Evaluate(CustomerRequest customerRequest)
    {
        // Evaluate the Customer and return the result
    }
}
```

Since it implements the `ICondition` interface, it must have the `Id` field, and the `Evaluate` method.

### 3) Register the services :
Lastly, you need to register the services in `Program.cs` file. You can just add the following lines : 
```cs
// Configurations
builder.Services.Configure<CustomConditionConfiguration>(
    builder.Configuration.GetSection("Application:Conditions:CustomCondition"));

// Services 
builder.Services.AddScoped<ICondition, CustomCondition>();
```

Now you can run the project and test the API, and in the response body, you will see the evaluation of your condition.

## Contributing 
Contributions are welcome! Follow these steps to contribute:
* Fork the project.
* Create a new branch: `git checkout -b feature/your-feature`.
* Make your changes.
* Submit a pull request.

## Thanks for your attention! 