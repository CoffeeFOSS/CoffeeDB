import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import {
  CreateRoasterDto,
  Roaster,
  RoasterRevisionDiff,
  RoasterRevisionSnapshot,
  RoasterSearchParams,
  UpdateRoasterDto,
} from '../models/roaster';
import { getHttpParams } from '../utils/params.utils';
import { RevisionMetadataExcerpt } from '../models/revision';
import { GenericSearchParams } from '../models/pagination';

@Injectable({
  providedIn: 'root',
})
export class RoastersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getRoasters(roasterSearchParams: RoasterSearchParams) {
    return this.http.get<Roaster[]>(`${this.baseUrl}roasters/`, {
      observe: 'response',
      params: getHttpParams(roasterSearchParams),
    });
  }

  getRoaster(id: number) {
    return this.http.get<Roaster>(`${this.baseUrl}roasters/${id}`);
  }

  createRoaster(createRoasterDto: CreateRoasterDto) {
    return this.http.post<RoasterRevisionSnapshot>(
      `${this.baseUrl}roasters/create`,
      createRoasterDto,
    );
  }

  updateRoaster(id: number, createRoasterRevisionDto: UpdateRoasterDto) {
    return this.http.post<RoasterRevisionSnapshot>(
      `${this.baseUrl}roasters/${id}/create-revision`,
      createRoasterRevisionDto,
    );
  }

  onDeleteRoaster(id: number) {
    return this.http.delete<void>(`${this.baseUrl}roasters/${id}`);
  }

  getRoasterRevisionSnapshot(id: number) {
    return this.http.get<RoasterRevisionSnapshot>(
      `${this.baseUrl}roasters/revisions/${id}`,
    );
  }

  updateRoasterRevision(
    revisionId: number,
    createRoasterRevisionDto: UpdateRoasterDto,
  ) {
    console.log(createRoasterRevisionDto, revisionId);
    return this.http.patch<RoasterRevisionSnapshot>(
      `${this.baseUrl}roasters/revisions/${revisionId}/edit`,
      createRoasterRevisionDto,
    );
  }

  getRoasterRevisionDiffs(
    roasterId: number,
    revisionId1: number,
    revisionId2: number | null,
  ) {
    return this.http.get<RoasterRevisionDiff>(
      `${this.baseUrl}roasters/${roasterId}/revisions/diff?revisionId1=${revisionId1}&revisionId2=${revisionId2}`,
    );
  }

  getRoasterRevisionMetadataExcerpts(
    // TODO: Params
    searchParams: GenericSearchParams,
    roasterId: number,
    committedOnly: boolean,
  ) {
    return this.http.get<RevisionMetadataExcerpt[]>(
      `${this.baseUrl}roasters/${roasterId}/revisions${committedOnly ? '?committedOnly=true' : ''}`,
      {
        observe: 'response',
        params: getHttpParams(searchParams),
      },
    );
  }
}
