import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from './client';
import type {
  ActivityDto, CreateLabelDto, CreateProjectDto, CreateTaskDto,
  LabelDto, ProjectDto, TaskDto, TodoStatus, UpdateTaskDto,
} from './types';

const k = {
  tasks: (params?: Record<string, string | undefined>) => ['tasks', params ?? {}] as const,
  task: (id: string) => ['task', id] as const,
  projects: ['projects'] as const,
  labels: ['labels'] as const,
  activity: ['activity'] as const,
  filter: (name: string) => ['filter', name] as const,
};

export function useTasks(params?: { projectId?: string; labelId?: string }) {
  const qs = new URLSearchParams();
  if (params?.projectId) qs.set('projectId', params.projectId);
  if (params?.labelId) qs.set('labelId', params.labelId);
  const suffix = qs.toString() ? `?${qs}` : '';
  return useQuery({
    queryKey: k.tasks(params as Record<string, string | undefined>),
    queryFn: () => api.get<TaskDto[]>(`/api/tasks${suffix}`),
  });
}

export function useSmartFilter(name: 'today' | 'overdue' | 'upcoming') {
  return useQuery({
    queryKey: k.filter(name),
    queryFn: () => api.get<TaskDto[]>(`/api/tasks/filters/${name}`),
  });
}

export function useProjects() {
  return useQuery({ queryKey: k.projects, queryFn: () => api.get<ProjectDto[]>('/api/projects') });
}

export function useLabels() {
  return useQuery({ queryKey: k.labels, queryFn: () => api.get<LabelDto[]>('/api/labels') });
}

export function useActivity() {
  return useQuery({
    queryKey: k.activity,
    queryFn: () => api.get<ActivityDto[]>('/api/activity?take=50'),
    refetchInterval: 5000,
  });
}

function invalidateAll(qc: ReturnType<typeof useQueryClient>) {
  qc.invalidateQueries({ queryKey: ['tasks'] });
  qc.invalidateQueries({ queryKey: ['filter'] });
  qc.invalidateQueries({ queryKey: ['activity'] });
}

export function useCreateTask() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (dto: CreateTaskDto) => api.post<TaskDto>('/api/tasks', dto),
    onSuccess: () => invalidateAll(qc),
  });
}

export function useUpdateTask() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, dto }: { id: string; dto: UpdateTaskDto }) =>
      api.put<TaskDto>(`/api/tasks/${id}`, dto),
    onSuccess: () => invalidateAll(qc),
  });
}

export function useUpdateTaskStatus() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, status }: { id: string; status: TodoStatus }) =>
      api.patch<TaskDto>(`/api/tasks/${id}/status`, { status }),
    onSuccess: () => invalidateAll(qc),
  });
}

export function useCompleteTask() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) =>
      api.post<{ completed: TaskDto; next: TaskDto | null }>(`/api/tasks/${id}/complete`, {}),
    onSuccess: () => invalidateAll(qc),
  });
}

export function useDeleteTask() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api.del(`/api/tasks/${id}`),
    onSuccess: () => invalidateAll(qc),
  });
}

export function useCreateProject() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (dto: CreateProjectDto) => api.post<ProjectDto>('/api/projects', dto),
    onSuccess: () => qc.invalidateQueries({ queryKey: k.projects }),
  });
}

export function useCreateLabel() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (dto: CreateLabelDto) => api.post<LabelDto>('/api/labels', dto),
    onSuccess: () => qc.invalidateQueries({ queryKey: k.labels }),
  });
}
