import { ComponentMeta, ComponentStory } from '@storybook/react';
import TodoItemInput from '../components-library/TodoItemInput';

export default {
  title: 'App/TodoItemInput',
  component: TodoItemInput,
} as ComponentMeta<typeof TodoItemInput>;

const Template: ComponentStory<typeof TodoItemInput> = (arg) => <TodoItemInput {...arg} />;

export const Empty = Template.bind({});
Empty.args = {
  currentValue: '',
};

export const Filled = Template.bind({});
Filled.args = {
  currentValue: 'foobar',
};
