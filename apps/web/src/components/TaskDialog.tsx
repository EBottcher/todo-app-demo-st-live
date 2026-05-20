import { useEffect, useRef, useState } from 'react';
import type {
  CreateTaskDto, LabelDto, ProjectDto, RecurrenceFrequency,
  TaskDto, TodoPriority, TodoStatus, UpdateTaskDto,
} from '../api/types';
import { useCreateTask, useUpdateTask } from '../api/hooks';

interface Props {
  task: TaskDto | null;            // null = creating new
  projects: ProjectDto[];
  labels: LabelDto[];
  onClose: () => void;
}

const PRIORITIES: TodoPriority[] = ['Low', 'Medium', 'High', 'Urgent'];
const STATUSES: TodoStatus[] = ['Todo', 'InProgress', 'Done'];
const FREQUENCIES: RecurrenceFrequency[] = ['Daily', 'Weekly', 'Monthly'];

function toLocalInput(iso?: string | null): string {
  if (!iso) return '';
  const d = new Date(iso);
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}
function fromLocalInput(v: string): string | null {
  return v ? new Date(v).toISOString() : null;
}

export function TaskDialog({ task, projects, labels, onClose }: Props) {
  const ref = useRef<HTMLDialogElement>(null);
  const create = useCreateTask();
  const update = useUpdateTask();

  const [title, setTitle] = useState(task?.title ?? '');
  const [description, setDescription] = useState(task?.description ?? '');
  const [status, setStatus] = useState<TodoStatus>(task?.status ?? 'Todo');
  const [priority, setPriority] = useState<TodoPriority>(task?.priority ?? 'Medium');
  const [projectId, setProjectId] = useState<string>(task?.projectId ?? '');
  const [selLabels, setSelLabels] = useState<string[]>(task?.labelIds ?? []);
  const [dueDate, setDueDate] = useState(toLocalInput(task?.dueDateUtc));
  const [reminder, setReminder] = useState(toLocalInput(task?.reminderUtc));
  const [recEnabled, setRecEnabled] = useState(!!task?.recurrence);
  const [freq, setFreq] = useState<RecurrenceFrequency>(task?.recurrence?.frequency ?? 'Daily');
  const [interval, setInterval] = useState<number>(task?.recurrence?.interval ?? 1);

  useEffect(() => { ref.current?.showModal(); }, []);

  const close = () => { ref.current?.close(); onClose(); };

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim()) return;
    const recurrence = recEnabled ? { frequency: freq, interval: Math.max(1, interval) } : null;
    const base = {
      title: title.trim(),
      description: description || null,
      priority,
      projectId: projectId || null,
      labelIds: selLabels,
      dueDateUtc: fromLocalInput(dueDate),
      reminderUtc: fromLocalInput(reminder),
      recurrence,
    };
    if (task) {
      const dto: UpdateTaskDto = { ...base, status };
      await update.mutateAsync({ id: task.id, dto });
    } else {
      const dto: CreateTaskDto = base;
      await create.mutateAsync(dto);
    }
    close();
  };

  return (
    <dialog ref={ref} onCancel={close}>
      <form onSubmit={submit}>
        <h2 style={{ marginTop: 0 }}>{task ? 'Edit task' : 'New task'}</h2>
        <div className="col">
          <label>Title</label>
          <input value={title} onChange={e => setTitle(e.target.value)} autoFocus required />
        </div>
        <div className="col" style={{ marginTop: 8 }}>
          <label>Description</label>
          <textarea value={description ?? ''} onChange={e => setDescription(e.target.value)} rows={3} />
        </div>
        <div className="row" style={{ marginTop: 8 }}>
          {task && (
            <div className="col" style={{ flex: 1 }}>
              <label>Status</label>
              <select value={status} onChange={e => setStatus(e.target.value as TodoStatus)}>
                {STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
              </select>
            </div>
          )}
          <div className="col" style={{ flex: 1 }}>
            <label>Priority</label>
            <select value={priority} onChange={e => setPriority(e.target.value as TodoPriority)}>
              {PRIORITIES.map(p => <option key={p} value={p}>{p}</option>)}
            </select>
          </div>
          <div className="col" style={{ flex: 1 }}>
            <label>Project</label>
            <select value={projectId} onChange={e => setProjectId(e.target.value)}>
              <option value="">(none)</option>
              {projects.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
            </select>
          </div>
        </div>
        <div className="row" style={{ marginTop: 8 }}>
          <div className="col" style={{ flex: 1 }}>
            <label>Due (local)</label>
            <input type="datetime-local" value={dueDate} onChange={e => setDueDate(e.target.value)} />
          </div>
          <div className="col" style={{ flex: 1 }}>
            <label>Reminder (local)</label>
            <input type="datetime-local" value={reminder} onChange={e => setReminder(e.target.value)} />
          </div>
        </div>
        <div className="col" style={{ marginTop: 8 }}>
          <label>Labels</label>
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
            {labels.map(l => {
              const checked = selLabels.includes(l.id);
              return (
                <button
                  type="button"
                  key={l.id}
                  onClick={() =>
                    setSelLabels(prev => prev.includes(l.id) ? prev.filter(x => x !== l.id) : [...prev, l.id])
                  }
                  style={{
                    background: checked ? l.color : 'white',
                    color: checked ? 'white' : l.color,
                    borderColor: l.color,
                  }}
                >
                  {l.name}
                </button>
              );
            })}
            {!labels.length && <span style={{ color: '#94a3b8', fontSize: 12 }}>No labels yet.</span>}
          </div>
        </div>
        <div style={{ marginTop: 12, padding: 12, background: '#f8fafc', borderRadius: 8 }}>
          <label style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <input type="checkbox" checked={recEnabled} onChange={e => setRecEnabled(e.target.checked)} style={{ width: 'auto' }} />
            Repeat
          </label>
          {recEnabled && (
            <div className="row" style={{ marginTop: 8 }}>
              <div className="col" style={{ flex: 1 }}>
                <label>Frequency</label>
                <select value={freq} onChange={e => setFreq(e.target.value as RecurrenceFrequency)}>
                  {FREQUENCIES.map(f => <option key={f} value={f}>{f}</option>)}
                </select>
              </div>
              <div className="col" style={{ width: 100 }}>
                <label>Every</label>
                <input type="number" min={1} value={interval} onChange={e => setInterval(Number(e.target.value))} />
              </div>
            </div>
          )}
        </div>
        <div className="row" style={{ marginTop: 16, justifyContent: 'flex-end' }}>
          <button type="button" onClick={close}>Cancel</button>
          <button type="submit" className="primary" disabled={create.isPending || update.isPending}>
            {task ? 'Save' : 'Create'}
          </button>
        </div>
      </form>
    </dialog>
  );
}
