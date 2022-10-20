import TodoApiClient from './TodoApiClient';

class TodoApis {
  async getTodos() {
    return await TodoApiClient.get('');
  }
}

export default new TodoApis();
