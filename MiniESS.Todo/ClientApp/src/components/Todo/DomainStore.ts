import { action, makeObservable, observable } from 'mobx';
import { TodoList } from '../../api/api';
import { notificationsUiStore } from '../Notifications/UiStore';
import TodoApi from './TodoApi';

export class DomainStore {
  @observable todoLists: TodoList[] = [];

  constructor() {
    makeObservable(this);
  }

  @action.bound
  async loadTodoLists() {
    const getTodosResult = await TodoApi.getTodos();
    this.updateTodoLists(getTodosResult.data.todoLists);
  }

  @action
  private updateTodoLists(todoLists: TodoList[]) {
    this.todoLists = todoLists;
  }

  @action.bound
  async completeTodoItem(todoId: string, todoItemId: number) {
    await TodoApi.completeTodoItem(todoId, todoItemId);
    await this.loadTodoLists();
    notificationsUiStore.addNotification('Todo item completed');
  }

  @action.bound
  async addNewTodoList(title: string) {
    await TodoApi.addTodo(title);
    await this.loadTodoLists();
    notificationsUiStore.addNotification('Todo list added');
  }

  @action.bound
  async onNewTodoItemAdded(todoId: string, name: string) {
    await TodoApi.addTodoItem(todoId, name);
    await this.loadTodoLists();
    notificationsUiStore.addNotification('Todo item added');
  }
}
