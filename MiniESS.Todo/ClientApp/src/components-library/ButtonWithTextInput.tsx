import { FC, useState } from 'react';
import { Button, InputGroup } from 'react-bootstrap';
import TextInput from './TextInput';

export type TodoItemInputProps = {
  onSubmit: (name: string) => Promise<void>;
  size?: 'sm' | 'lg';
  buttonText?: string;
};

const ButtonWithTextInput: FC<TodoItemInputProps> = (props: TodoItemInputProps) => {
  const { size, buttonText } = props;
  const [currentValue, setCurrentValue] = useState<string>('');
  return (
    <InputGroup>
      <Button
        {...(size ? { size: size } : {})}
        variant="primary"
        disabled={currentValue.length == 0}
        onClick={async () => {
          await props.onSubmit(currentValue);
          setCurrentValue('');
        }}
      >
        {buttonText ?? 'Add'}
      </Button>
      <TextInput
        size={size}
        isInvalid={false}
        isValid={false}
        disabled={false}
        onChanged={(value: string) => setCurrentValue(value)}
        value={currentValue}
      />
    </InputGroup>
  );
};

export default ButtonWithTextInput;
