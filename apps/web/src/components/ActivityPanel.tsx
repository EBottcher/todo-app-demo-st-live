import { useActivity } from '../api/hooks';

export function ActivityPanel() {
  const { data } = useActivity();
  return (
    <aside className="activity">
      <h3>Activity</h3>
      {(data ?? []).map(a => (
        <div key={a.id} className="entry">
          <div><strong>{a.action}</strong> — {a.summary ?? a.entityType}</div>
          <div className="time">{new Date(a.timestampUtc).toLocaleString()} · {a.actor}</div>
        </div>
      ))}
      {!data?.length && <p style={{ color: '#94a3b8' }}>No activity yet.</p>}
    </aside>
  );
}
