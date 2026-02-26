// TypeScript DTOs matching backend domain entities

export interface Patient {
  id: string;
  pseudonymizedId: string;
  dateOfBirth: string; // ISO date string YYYY-MM-DD
  sex: string;
  siteId: string;
  createdAt: string;
  updatedAt: string;
}

export interface Site {
  id: string;
  name: string;
  country: string;
  isActive: boolean;
  createdAt: string;
}

export interface ClinicalVisit {
  id: string;
  patientId: string;
  visitDate: string; // ISO date string YYYY-MM-DD
  bestCorrectedVisualAcuity?: string;
  intraocularPressure?: string;
  clinicalNotes?: string;
  createdAt: string;
}

export interface ImagingStudy {
  id: string;
  clinicalVisitId: string;
  modality: string;
  orthancStudyId?: string;
  minioObjectKey?: string;
  acquiredAt: string;
  createdAt: string;
}

export interface AuditLog {
  id: number;
  userId: string;
  action: string;
  entityType: string;
  entityId?: string;
  details?: string;
  ipAddress: string;
  occurredAt: string;
}

export type UserRole = "SiteAdmin" | "Clinician" | "Researcher" | "SystemAdmin";

export interface ApiError {
  message: string;
  code?: string;
  details?: Record<string, string[]>;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
