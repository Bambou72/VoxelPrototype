
namespace Crecerelle.Constraints
{ 
    public struct ConstraintHolder
    { 
        public bool Constrained 
        { 
            get {
                return XConstraint!= null|| YConstraint != null || WidthConstraint != null || HeightConstraint != null ;
            } 
        }
        public IConstraint XConstraint;
        public IConstraint YConstraint;
        public IConstraint WidthConstraint;
        public IConstraint HeightConstraint;
    }
}
