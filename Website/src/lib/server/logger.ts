import bunyan, { type Stream } from 'bunyan';
import seq from 'bunyan-seq';

import { env } from '$env/dynamic/private';

const loggingStreams: Stream[] = [
  {
    stream: process.stdout,
    level: 'debug'
  },
  ...(env.SEQ_URL
    ? [
        seq.createStream({
          serverUrl: env.SEQ_URL,
          level: 'debug',
          apiKey: env.SEQ_API_KEY
        })
      ]
    : [])
];

const logger = bunyan.createLogger({
  name: 'dawnshard',
  streams: loggingStreams
});

const createLogger = (sourceContext: string) => logger.child({ sourceContext });

export default createLogger;
