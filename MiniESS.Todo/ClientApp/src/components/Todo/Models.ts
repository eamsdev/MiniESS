import { TodoItem, TodoList } from '../../api/api';

export class TodoListViewModel {
  id: string;
  title: string;
  todoItems: TodoItemViewModel[];

  constructor(id: string, title: string, todoItems: TodoItemViewModel[]) {
    this.id = id;
    this.title = title;
    this.todoItems = todoItems;
  }

  static From(model: TodoList, onComplete: (todoId: string, itemId: number) => Promise<void>) {
    const partialOnComplete = async (itemId: number) => await onComplete(model.StreamId, itemId);
    const todoItems = model.TodoItems.map((x) => TodoItemViewModel.From(x, partialOnComplete));
    return new TodoListViewModel(model.StreamId, model.Title, todoItems);
  }
}

export class TodoItemViewModel {
  id: number;
  isCompleted: boolean;
  description: string;
  onComplete: () => Promise<void>;

  constructor(
    id: number,
    isCompleted: boolean,
    description: string,
    onComplete: (itemId: number) => Promise<void>,
  ) {
    this.id = id;
    this.isCompleted = isCompleted;
    this.description = description;
    this.onComplete = async () => await onComplete(id);
  }

  static From(model: TodoItem, onComplete: (itemId: number) => Promise<void>) {
    return new TodoItemViewModel(model.Id, model.IsCompleted, model.Description, onComplete);
  }
}
