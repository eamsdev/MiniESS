import {
  GetTodoListsViewModel,
  GetTodoListViewModel,
  AddTodoListResponseModel,
  AddTodoListInputModel,
  AddTodoItemResponseModel,
  AddTodoItemInputModel,
  CompleteTodoItemResponseModel,
  CompleteTodoItemInputModel,
} from './../../api/api';
import TodoApiClient from './TodoApiClient';

class TodoApis {
  async getTodos() {
    return await TodoApiClient.get<GetTodoListsViewModel>('');
  }

  async getTodo(todoListId: string) {
    return await TodoApiClient.get<GetTodoListViewModel>(`/${todoListId}`);
  }

  async addTodo(title: string) {
    const data: AddTodoListInputModel = {
      title: title,
    };

    return await TodoApiClient.post<AddTodoListResponseModel>('', data);
  }

  async addTodoItem(todoListId: string, description: string) {
    const data: AddTodoItemInputModel = {
      todoListId: todoListId,
      description: description,
    };

    return await TodoApiClient.post<AddTodoItemResponseModel>(`/${todoListId}/items`, data);
  }

  async completeTodoItem(todoListId: string, todoItemId: number) {
    const data: CompleteTodoItemInputModel = {
      todoListId: todoListId,
      todoItemId: todoItemId,
    };

    return await TodoApiClient.put<CompleteTodoItemResponseModel>(
      `/${todoListId}/items/${todoItemId}`,
      data,
    );
  }
}

export default new TodoApis();
