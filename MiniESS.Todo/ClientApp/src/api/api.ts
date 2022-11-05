//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.17.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming



export interface GetTodoListsViewModel {
    TodoLists: TodoListViewModel[];
}

export interface TodoListViewModel {
    StreamId: string;
    Title: string;
    TodoItems: TodoItemViewModel[];
}

export interface TodoItemViewModel {
    Id: number;
    Order: number;
    IsCompleted: boolean;
    Description: string;
}

export interface AddTodoListResponseModel {
    CreatedTodoListId: string;
}

export interface AddTodoListInputModel {
    Title: string;
}

export interface GetTodoListViewModel {
    TodoList: TodoListViewModel;
}

export interface WeatherForecast {
    Date: Date;
    TemperatureC: number;
    TemperatureF: number;
    Summary: string;
}