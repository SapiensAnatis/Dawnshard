const renderDate = (date: Date | null) => {
  if (!date || date.valueOf() < new Date(1970, 1, 1).valueOf()) {
    return 'Never!';
  }

  return date.toLocaleString(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short'
  });
};

export default renderDate;
