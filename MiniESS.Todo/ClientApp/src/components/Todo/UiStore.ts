import { TodoListModel } from './Models';
import { action, computed, makeObservable } from 'mobx';
import { DomainStore } from './DomainStore';

export default class UiStore {
  domainStore: DomainStore;

  constructor() {
    makeObservable(this);
    this.domainStore = new DomainStore();
  }

  @action
  async onNavigate() {
    await this.domainStore.loadTodoLists();
  }

  @computed
  get todoListViewModels() {
    return this.domainStore.todoLists.map((todoList) =>
      TodoListModel.From(todoList, this.domainStore.completeTodoItem),
    );
  }
}
