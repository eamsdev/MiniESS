import { TodoListModel } from './Models';
import { action, computed, makeObservable } from 'mobx';
import { DomainStore } from './DomainStore';

export class UiStore {
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
  get todoListsProps() {
    return this.domainStore.todoLists.map((todoList) =>
      TodoListModel.From(todoList).toPropsModel(
        this.domainStore.onNewTodoItemAdded,
        this.domainStore.completeTodoItem,
      ),
    );
  }
}

const todoUiStore = new UiStore();
export { todoUiStore };
