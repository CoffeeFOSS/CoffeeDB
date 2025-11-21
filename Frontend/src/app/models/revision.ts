import { RevisionStatus } from './roaster';

export interface RevisionMetadataExcerpt {
  id: number;
  comment: string;
  status: RevisionStatus;
  createdAt: null | string;
  updatedAt: null | string;
  createdBy: null | string;
  version: null | number;
  parentRevisionId?: number;
}

export interface RevisionMetadata extends RevisionMetadataExcerpt {
  entityType: string;
}

export interface RevisionMetadataContribution extends RevisionMetadata {
  entityName: string;
  entityId: string;
}
