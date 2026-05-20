import { NavLink } from 'react-router-dom';
import { useState } from 'react';
import { useCreateLabel, useCreateProject, useLabels, useProjects } from '../api/hooks';

interface Props {
  theme: 'light' | 'dark';
  onToggleTheme: () => void;
}

export function Sidebar({ theme, onToggleTheme }: Props) {
  const projects = useProjects();
  const labels = useLabels();
  const createProject = useCreateProject();
  const createLabel = useCreateLabel();
  const [pName, setPName] = useState('');
  const [lName, setLName] = useState('');

  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <h2 style={{ marginTop: 0, marginBottom: 0 }}>TodoApp</h2>
        <button type="button" className="theme-toggle" onClick={onToggleTheme}>
          {theme === 'dark' ? '☀️ Light' : '🌙 Dark'}
        </button>
      </div>

      <h3>Smart filters</h3>
      <NavLink to="/" end>📥 All</NavLink>
      <NavLink to="/filter/today">📅 Today</NavLink>
      <NavLink to="/filter/overdue">⚠️ Overdue</NavLink>
      <NavLink to="/filter/upcoming">⏭ Upcoming</NavLink>

      <h3>Projects</h3>
      {(projects.data ?? []).map(p => (
        <NavLink key={p.id} to={`/project/${p.id}`}>
          <span className="swatch" style={{ background: p.color }} />{p.name}
        </NavLink>
      ))}
      <form
        style={{ display: 'flex', gap: 4, marginTop: 8 }}
        onSubmit={e => {
          e.preventDefault();
          if (!pName.trim()) return;
          createProject.mutate({ name: pName.trim() });
          setPName('');
        }}
      >
        <input placeholder="New project" value={pName} onChange={e => setPName(e.target.value)} />
        <button type="submit">+</button>
      </form>

      <h3>Labels</h3>
      {(labels.data ?? []).map(l => (
        <NavLink key={l.id} to={`/label/${l.id}`}>
          <span className="swatch" style={{ background: l.color }} />{l.name}
        </NavLink>
      ))}
      <form
        style={{ display: 'flex', gap: 4, marginTop: 8 }}
        onSubmit={e => {
          e.preventDefault();
          if (!lName.trim()) return;
          createLabel.mutate({ name: lName.trim() });
          setLName('');
        }}
      >
        <input placeholder="New label" value={lName} onChange={e => setLName(e.target.value)} />
        <button type="submit">+</button>
      </form>
    </aside>
  );
}
