import { RevisionStatus } from './roaster';

export interface RevisionMetadataExcerpt {
  id: number;
  comment: string;
  status: RevisionStatus;
  createdBy?: string;
  version?: number;
  parentRevisionId?: number;
}
