import type { TaskDto, ProjectDto, LabelDto } from '../api/types';
import {
  useCompleteTask, useDeleteTask, useUpdateTaskStatus,
} from '../api/hooks';

interface Props {
  tasks: TaskDto[];
  projects: ProjectDto[];
  labels: LabelDto[];
  onEdit: (task: TaskDto) => void;
}

export function TaskList({ tasks, projects, labels, onEdit }: Props) {
  const toggle = useUpdateTaskStatus();
  const complete = useCompleteTask();
  const del = useDeleteTask();

  if (!tasks.length) return <p style={{ color: '#94a3b8' }}>No tasks here.</p>;

  const projectMap = new Map(projects.map(p => [p.id, p]));
  const labelMap = new Map(labels.map(l => [l.id, l]));
  const now = Date.now();

  return (
    <div className="tasklist">
      {tasks.map(t => {
        const due = t.dueDateUtc ? new Date(t.dueDateUtc) : null;
        const overdue = due && t.status !== 'Done' && due.getTime() < now;
        const project = t.projectId ? projectMap.get(t.projectId) : null;
        return (
          <div key={t.id} className={`task ${t.status === 'Done' ? 'done' : ''}`}>
            <input
              type="checkbox"
              checked={t.status === 'Done'}
              onChange={e => {
                if (e.target.checked) complete.mutate(t.id);
                else toggle.mutate({ id: t.id, status: 'Todo' });
              }}
            />
            <div style={{ flex: 1, cursor: 'pointer' }} onClick={() => onEdit(t)}>
              <div className="title">{t.title}</div>
              {t.description && <div style={{ color: '#64748b', fontSize: 13 }}>{t.description}</div>}
              <div className="meta">
                <span className={`badge priority-${t.priority}`}>{t.priority}</span>
                {project && (
                  <span className="badge" style={{ background: project.color + '33', color: project.color }}>
                    {project.name}
                  </span>
                )}
                {t.labelIds.map(id => {
                  const l = labelMap.get(id);
                  return l ? (
                    <span key={id} className="badge" style={{ background: l.color + '33', color: l.color }}>
                      {l.name}
                    </span>
                  ) : null;
                })}
                {due && (
                  <span className={`badge ${overdue ? 'overdue' : ''}`}>
                    {overdue ? 'Overdue ' : ''}{due.toLocaleString()}
                  </span>
                )}
                {t.recurrence && (
                  <span className="badge">↻ {t.recurrence.frequency} ×{t.recurrence.interval}</span>
                )}
                {t.reminderUtc && (
                  <span className="badge">⏰ {new Date(t.reminderUtc).toLocaleString()}</span>
                )}
              </div>
            </div>
            <button onClick={() => del.mutate(t.id)} title="Delete">✕</button>
          </div>
        );
      })}
    </div>
  );
}
