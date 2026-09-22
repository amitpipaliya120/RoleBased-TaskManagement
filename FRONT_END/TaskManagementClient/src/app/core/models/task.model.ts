export interface Task {
  taskId?: number;
  title?: string;
  description?: string;
  status?: string;
  priority?: string;
  assigneeId?: number;
  managerId?: number;
  deadline?: string;
  assigneeName?: string;
  managerName?: string;
}
