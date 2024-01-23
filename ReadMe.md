# SimpleComponents
[Simple Components](https://github.com/phamthehung-NET/SimpleComponents) is a collection of easy to use and implement [Razor components class libraries](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries?view=aspnetcore-7.0&tabs=visual-studio)

This project includes components:
* Calendar

### Setup

Add reference to style sheet & javascript references
Add the following line to the head tag of your _Host.cshtml (Blazor Server).
If you already imported bootstrap v5.1 for your project, you can pass through this step

#### Head Inclusions
```html
<link href="./_content/SimpleComponents/css/bootstrap.min.css" rel="stylesheet" />
```
##### Example: (head)
```html
    ....
    <link href="{YourBlazorProject}.styles.css" rel="stylesheet" />
    <link href="./_content/SimpleComponents/css/bootstrap.min.css" rel="stylesheet" />
</head>

```


Then add a reference to the Calendar JavaScript file at the bottom of the respective page after the reference to the Blazor file.
#### Body Inclusions
```html
<script src="./_content/SimpleComponents/js/bootstrap.min.js"></script>
```

##### Example: (body)
```html
    ....
    <script src="_framework/blazor.webassembly.js"></script>
    <script src="./_content/SimpleComponents/js/bootstrap.min.js"></script>
</body>
```

### Usage
SimpleComponents will update the bound content variable on change in the editor.

``` html
@page "/"

<SimpleComponents.Calendar T="ObjectDTO"
                    Context="Context"
                    Events="Events"
                    MinDate="DateTime.Now"
                    ShowedDate="DateTime.Now"
                    HighLightToday="true"
                    StartDayOfWeek="DayOfWeek.Monday">
</SimpleComponents.Calendar> 

<h2>@content</h2>

@code{
    private string content;
}
```

## ToDo
Add additional configuration and add drag calendar events handling. Will be updated in v0.0.2