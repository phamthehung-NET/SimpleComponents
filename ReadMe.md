# SimpleComponents
[Simple Components](https://github.com/phamthehung-NET/SimpleComponents) is a collection of easy to use and implement [Razor components class libraries](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/class-libraries?view=aspnetcore-7.0&tabs=visual-studio)

This project includes components:
* Calendar
* TreeView
* FolderView

# Features
### Calendar
* Display events on the grid
* Customize the event content
* Provide handling method when click to event on calendar
* Drag to select date range
* Provide handling method when user click to a date on grid
* Embedded data inside event
### Tree view
* Display a tree with item
* Customize each tree item content
* Provide handling method when click to item on tree
* Embedded data inside item
### Folder view
* Showing folders as folder tree
* Provide handling method when click to each folder
* Provide handling method drag and drop to move the folders and their children on folder tree (drag other item to folder will trigger this method as well)
* Embedded data inside folder

# Demo screenshot
### Calendar
![Screenshot](https://github.com/phamthehung-NET/SimpleComponents/blob/master/wwwroot/demo.png)
### Horizontal Tree View
![Screenshot](https://github.com/phamthehung-NET/SimpleComponents/blob/master/wwwroot/treeview-demo.png)
### Folder view
![Screenshot](https://github.com/phamthehung-NET/SimpleComponents/blob/master/wwwroot/folderview-demo.png)

### Setup

Add reference to style sheet & javascript references
Add the following line to the head tag of your _Host.cshtml (Blazor Server).
(new update for .NET 8 - add import to App.razor file)
If you have already imported bootstrap v5.1 for your project, you can skip this step

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
If you have already imported bootstrap v5.1 and jquery v3.7 into your project, you can skip this step
#### Body Inclusions
```html
<script src="./_content/SimpleComponents/js/bootstrap.bundle.js"></script>
<script src="./_content/SimpleComponents/js/jquery-3.7.0.min.js"></script>
```

##### Example: (body)
```html
    ....
    <script src="_framework/blazor.webassembly.js"></script>
    <script src="./_content/SimpleComponents/js/bootstrap.bundle.js"></script>
    <script src="./_content/SimpleComponents/js/jquery-3.7.0.min.js"></script>
</body>
```

### Usage
#### Calendar
SimpleComponents.Calendar will update the bound Events variable on change on the grid.

``` html
@page "/"

<SimpleComponents.Calendar T="TestData"
                    Context="Context"
                    Events="Events"
                    MinDate="DateTime.Now"
                    ShowedDate="new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)"
                    HighLightToday="true"
                    EventTodayBackGroundColor="#BAF3FF"
                    StartDayOfWeek="DayOfWeek.Monday">
                    <EventContentTemplate>
                        <div>• @Context.Name</div>
                        <div>• @Context.StartDate.ToString("dd/MM") ~ @Context.EndDate.ToString("dd/MM")</div>
                    </EventContentTemplate>
</SimpleComponents.Calendar> 

<h2>@content</h2>

@code{
    private string content;

    List<CalendarEvent<TestData>> Events = new()
    {
        new()
        {
            Id = 1,
            Data = new()
            {
                Name = "Event 1",
                StartDate = new DateTime(2024, 1, 3),
                EndDate = new DateTime(2024, 1, 12),
            },
            StartDate = new DateTime(2024, 1, 3),
            EndDate = new DateTime(2024, 1, 12),
        },
        ...
    };
}
```

#### TreeView
SimpleComponents.TreeView will send the tree item data when you click to each item on the tree view

``` html
@page "/"

<SimpleComponents.TreeView T="TestData" TreeItems="TreeItems" />

<h2>@content</h2>

@code{
    private string content;

    List<TreeViewModel<TestData>> TreeItems = new()
    {
        new()
        {
            Id = 1,
            Data = new()
            {
                Name = "Item level 1"
            },
            Level = 1,
            ParentId = 0,
            Content = "Item level 1",
        },
        ...
    };
}
```

#### FolderView
SimpleComponents.FolderView will send the folder object data when you click to each folder on the tree view and collapse/expand folder at the same time

``` html
@page "/"

<SimpleComponents.FolderView T="TestData" Folders="FolderItems" />

<h2>@content</h2>

@code{
    private string content;

    List<FolderModel<TestData>> FolderItems = new()
    {
        new()
        {
            Id = 1,
            Data = new()
            {
                Name = "Folder level 1"
            },
            Level = 1,
            ParentId = 0,
            Content = "Folder level 1",
        },
        ...
    };
}
```

## ToDo
Add additional configuration.

Add new Components

Gives me some ideas about essential components that will be developed in the future

Contact <a href="mailto:phamthehung.1.2@outlook.com">me</a> or <a href="mailto:phamthehung010221@gmail.com">me in Gmail</a> if you have any bugs or contribute ideas for new components

## Versions histories
* v0.0.1 Calendar view
* v0.0.2 Calendar events interaction
* v0.0.3 Fixing flexible line of calendar each calendar event
* v0.0.4 Adding TreeView and FolderView components
* v0.0.4.1 Fixing FolderView Css
* v0.0.4.2 Fixing failed to import Jquery
* v0.0.4.3 Adding readme.md to Nuget
* v0.0.4.7 Fixing showing events on calendar
* v0.0.4.8 Fixing ReadMe.md