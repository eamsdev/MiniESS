import { ComponentMeta, ComponentStory } from '@storybook/react';
import TodoItem from '../components-library/TodoItem';

export default {
  title: 'App/TodoItem',
  component: TodoItem,
} as ComponentMeta<typeof TodoItem>;

const Template: ComponentStory<typeof TodoItem> = (arg) => <TodoItem {...arg} />;

export const Unchecked = Template.bind({});
Unchecked.args = {
  isChecked: false,
  label: 'foobar',
};

export const Checked = Template.bind({});
Checked.args = {
  isChecked: true,
  label: 'foobar',
};
