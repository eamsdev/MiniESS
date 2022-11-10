import { action, makeObservable, observable, runInAction } from 'mobx';
import { TodoList } from '../../api/api';
import TodoApi from './TodoApi';

export class DomainStore {
  @observable todoLists: TodoList[] = [];

  constructor() {
    makeObservable(this);
  }

  @action.bound
  async loadTodoLists() {
    const getTodosResult = await TodoApi.getTodos();
    this.todoLists = getTodosResult.data.TodoLists;
  }

  @action.bound
  async completeTodoItem(todoId: string, todoItemId: number) {
    await TodoApi.completeTodoItem(todoId, todoItemId);
    runInAction(async () => {
      await this.loadTodoLists();
    });
  }

  @action.bound
  async addNewTodoList(title: string) {
    await TodoApi.addTodo(title);
    runInAction(async () => {
      await this.loadTodoLists();
    });
  }

  @action.bound
  async onNewTodoItemAdded(todoId: string, name: string) {
    await TodoApi.addTodoItem(todoId, name);
    runInAction(async () => {
      await this.loadTodoLists();
    });
  }
}
