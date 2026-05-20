import { useState } from 'react';
import { Route, Routes, useParams } from 'react-router-dom';
import { Sidebar } from './components/Sidebar';
import { ActivityPanel } from './components/ActivityPanel';
import { TaskList } from './components/TaskList';
import { TaskDialog } from './components/TaskDialog';
import {
  useLabels, useProjects, useSmartFilter, useTasks,
} from './api/hooks';
import type { TaskDto } from './api/types';

type DialogState = { open: false } | { open: true; task: TaskDto | null };

function useDialog() {
  const [s, set] = useState<DialogState>({ open: false });
  return {
    state: s,
    openNew: () => set({ open: true, task: null }),
    openEdit: (t: TaskDto) => set({ open: true, task: t }),
    close: () => set({ open: false }),
  };
}

function Pane({ title, data, dialog }: {
  title: string;
  data: TaskDto[] | undefined;
  dialog: ReturnType<typeof useDialog>;
}) {
  const projects = useProjects();
  const labels = useLabels();
  return (
    <>
      <div className="toolbar">
        <h1 style={{ margin: 0, flex: 1 }}>{title}</h1>
        <button className="primary" onClick={dialog.openNew}>+ New task</button>
      </div>
      <TaskList
        tasks={data ?? []}
        projects={projects.data ?? []}
        labels={labels.data ?? []}
        onEdit={dialog.openEdit}
      />
    </>
  );
}

function AllView({ dialog }: { dialog: ReturnType<typeof useDialog> }) {
  const { data } = useTasks();
  return <Pane title="All tasks" data={data} dialog={dialog} />;
}

function FilterView({ dialog }: { dialog: ReturnType<typeof useDialog> }) {
  const { name } = useParams<{ name: 'today' | 'overdue' | 'upcoming' }>();
  const { data } = useSmartFilter(name!);
  const titles = { today: 'Today', overdue: 'Overdue', upcoming: 'Upcoming' } as const;
  return <Pane title={titles[name!]} data={data} dialog={dialog} />;
}

function ProjectView({ dialog }: { dialog: ReturnType<typeof useDialog> }) {
  const { id } = useParams<{ id: string }>();
  const projects = useProjects();
  const { data } = useTasks({ projectId: id });
  const p = projects.data?.find(x => x.id === id);
  return <Pane title={p?.name ?? 'Project'} data={data} dialog={dialog} />;
}

function LabelView({ dialog }: { dialog: ReturnType<typeof useDialog> }) {
  const { id } = useParams<{ id: string }>();
  const labels = useLabels();
  const { data } = useTasks({ labelId: id });
  const l = labels.data?.find(x => x.id === id);
  return <Pane title={`# ${l?.name ?? 'Label'}`} data={data} dialog={dialog} />;
}

export default function App() {
  const dialog = useDialog();
  const projects = useProjects();
  const labels = useLabels();

  return (
    <div className="app">
      <Sidebar />
      <main className="main">
        <Routes>
          <Route path="/" element={<AllView dialog={dialog} />} />
          <Route path="/filter/:name" element={<FilterView dialog={dialog} />} />
          <Route path="/project/:id" element={<ProjectView dialog={dialog} />} />
          <Route path="/label/:id" element={<LabelView dialog={dialog} />} />
        </Routes>
      </main>
      <ActivityPanel />
      {dialog.state.open && (
        <TaskDialog
          task={dialog.state.task}
          projects={projects.data ?? []}
          labels={labels.data ?? []}
          onClose={dialog.close}
        />
      )}
    </div>
  );
}
