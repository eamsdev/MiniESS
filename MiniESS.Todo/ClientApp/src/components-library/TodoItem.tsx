import { faSquareCheck } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { FC } from 'react';
import { Stack } from 'react-bootstrap';

export type TodoItemProps = {
  id: number;
  isCompleted: boolean;
  description: string;
  onComplete: (id: number) => Promise<void>;
};

const TodoItem: FC<TodoItemProps> = (props: TodoItemProps) => {
  return (
    <Stack direction="horizontal" gap={2}>
      <>
        {props.isCompleted ? (
          <>{props.description}</>
        ) : (
          <del className="completed-label">{props.description}</del>
        )}
      </>
      {props.isCompleted && (
        <a onClick={async () => await props.onComplete(props.id)}>
          <FontAwesomeIcon icon={faSquareCheck} />
        </a>
      )}
    </Stack>
  );
};

export default TodoItem;
