import { ComponentMeta, ComponentStory } from '@storybook/react';
import TodoList from '../components-library/TodoList';

export default {
  title: 'App/TodoList',
  component: TodoList,
} as ComponentMeta<typeof TodoList>;

const Template: ComponentStory<typeof TodoList> = (arg) => <TodoList {...arg} />;

export const Default = Template.bind({});
Default.args = {
  title: 'FooBarTodoList',
  items: [
    {
      isChecked: true,
      label: 'FooItem',
    },
    {
      isChecked: false,
      label: 'BarItem',
    },
    {
      isChecked: true,
      label: 'BarfooItem',
    },
    {
      isChecked: false,
      label: 'FoobarItem',
    },
  ],
};
