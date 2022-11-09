import { TodoListProps } from './../../components-library/TodoList';
import { TodoItem, TodoList } from '../../api/api';
import { TodoItemProps } from '../../components-library/TodoItem';

export class TodoListModel {
  id: string;
  title: string;
  todoItems: TodoItemModel[];

  constructor(id: string, title: string, todoItems: TodoItemModel[]) {
    this.id = id;
    this.title = title;
    this.todoItems = todoItems;
  }

  static From(model: TodoList) {
    const todoItems = model.TodoItems.map((x) => TodoItemModel.From(x));
    return new TodoListModel(model.StreamId, model.Title, todoItems);
  }

  toPropsModel(
    onNewItemAdded: (name: string) => Promise<void>,
    onComplete: (todoId: string, itemId: number) => Promise<void>,
  ): TodoListProps {
    return {
      title: this.title,
      items: this.todoItems.map((x) =>
        x.toPropModel(async (id: number) => await onComplete(this.id, id)),
      ),
      onSubmit: onNewItemAdded,
    };
  }
}

export class TodoItemModel {
  id: number;
  isCompleted: boolean;
  description: string;

  constructor(id: number, isCompleted: boolean, description: string) {
    this.id = id;
    this.isCompleted = isCompleted;
    this.description = description;
  }

  static From(model: TodoItem) {
    return new TodoItemModel(model.Id, model.IsCompleted, model.Description);
  }

  toPropModel(onComplete: (id: number) => Promise<void>): TodoItemProps {
    return {
      id: this.id,
      isCompleted: this.isCompleted,
      description: this.description,
      onComplete: onComplete,
    };
  }
}
