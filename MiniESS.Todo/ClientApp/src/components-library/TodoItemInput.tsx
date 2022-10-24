import { FC, useState } from 'react';
import { Button, InputGroup } from 'react-bootstrap';
import TextInput from './TextInput';

export type TodoItemInputProps = {
  onSubmit: (name: string) => Promise<void>;
};

const TodoItemInput: FC<TodoItemInputProps> = (props: TodoItemInputProps) => {
  const [currentValue, setCurrentValue] = useState<string>('');
  return (
    <InputGroup>
      <Button
        variant="primary"
        disabled={currentValue.length == 0}
        onClick={async () => {
          await props.onSubmit(currentValue);
          setCurrentValue('');
        }}
      >
        Add
      </Button>
      <TextInput
        isInvalid={false}
        isValid={false}
        disabled={false}
        onChanged={(value: string) => setCurrentValue(value)}
        value={currentValue}
      />
    </InputGroup>
  );
};

export default TodoItemInput;
