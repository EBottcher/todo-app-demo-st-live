export type TodoStatus = 'Todo' | 'InProgress' | 'Done';
export type TodoPriority = 'Low' | 'Medium' | 'High' | 'Urgent';
export type RecurrenceFrequency = 'Daily' | 'Weekly' | 'Monthly';
export type ActivityAction =
  | 'Created' | 'Updated' | 'StatusChanged' | 'Completed'
  | 'Recurred' | 'Deleted' | 'ReminderFired';

export interface RecurrenceRule {
  frequency: RecurrenceFrequency;
  interval: number;
  endDateUtc?: string | null;
  count?: number | null;
}

export interface TaskDto {
  id: string;
  title: string;
  description?: string | null;
  status: TodoStatus;
  priority: TodoPriority;
  projectId?: string | null;
  labelIds: string[];
  dueDateUtc?: string | null;
  reminderUtc?: string | null;
  reminderFired: boolean;
  recurrence?: RecurrenceRule | null;
  createdUtc: string;
  updatedUtc: string;
  completedUtc?: string | null;
}

export interface CreateTaskDto {
  title: string;
  description?: string | null;
  priority?: TodoPriority;
  projectId?: string | null;
  labelIds?: string[];
  dueDateUtc?: string | null;
  reminderUtc?: string | null;
  recurrence?: RecurrenceRule | null;
}

export interface UpdateTaskDto {
  title: string;
  description?: string | null;
  status: TodoStatus;
  priority: TodoPriority;
  projectId?: string | null;
  labelIds: string[];
  dueDateUtc?: string | null;
  reminderUtc?: string | null;
  recurrence?: RecurrenceRule | null;
}

export interface ProjectDto { id: string; name: string; color: string; createdUtc: string; }
export interface CreateProjectDto { name: string; color?: string; }

export interface LabelDto { id: string; name: string; color: string; }
export interface CreateLabelDto { name: string; color?: string; }

export interface ActivityDto {
  id: string;
  entityType: string;
  entityId: string;
  action: ActivityAction;
  actor: string;
  timestampUtc: string;
  summary?: string | null;
  changeJson?: string | null;
}
