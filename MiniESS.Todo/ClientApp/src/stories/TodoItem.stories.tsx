import { ComponentMeta, ComponentStory } from '@storybook/react';
import TodoItem from '../components-library/TodoItem';

export default {
  title: 'App/TodoItem',
  component: TodoItem,
} as ComponentMeta<typeof TodoItem>;

const Template: ComponentStory<typeof TodoItem> = (arg) => <TodoItem {...arg} />;

export const Default = Template.bind({});
Default.args = {
  todoItemId: '2aj3f3g',
  isChecked: true,
  label: 'foobar',
};
