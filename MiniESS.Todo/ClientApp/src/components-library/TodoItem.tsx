import { faSquareCheck, faTimesSquare } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { FC } from 'react';
import { Stack } from 'react-bootstrap';

export type TodoItemProps = {
  todoItemId: string;
  isChecked: boolean;
  label: string;
  onComplete: (value: string) => Promise<void>;
};

const TodoItem: FC<TodoItemProps> = (props: TodoItemProps) => {
  return (
    <Stack direction="horizontal" gap={2}>
      <span>
        {props.isChecked ? (
          <>{props.label}</>
        ) : (
          <del className="completed-label">{props.label}</del>
        )}
      </span>
      {props.isChecked && (
        <a onClick={async () => await props.onComplete(props.todoItemId)}>
          <FontAwesomeIcon icon={faSquareCheck} />
        </a>
      )}
    </Stack>
  );
};

export default TodoItem;
