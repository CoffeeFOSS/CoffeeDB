export function getReadableDate(dateIsoString: string | null): string | null {
  if (!dateIsoString) return null;
  const date = new Date(dateIsoString);

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  let hours = date.getHours();
  const ampm = hours >= 12 ? 'PM' : 'AM';
  hours = hours % 12 || 12; // convert to 12-hour format
  const minutes = String(date.getMinutes()).padStart(2, '0');

  return `${year}-${month}-${day} at ${hours}:${minutes} ${ampm}`;
}
